using InventorySystem.DatabaseContext;
using InventorySystem.Models;
using InventorySystem.Repositories;
using InventorySystem.ViewModel;
using Microsoft.EntityFrameworkCore;
namespace InventorySystem.Repositories;
public interface ICustomerPaymentRepository
{
    Task<List<CustomerPayment>> GetAllPaymentsAsync(long? customerId = null,long? invoiceId = null);

    Task<List<CustomerLedger>> GetCustomerLedgerAsync(long? customerId = null,long? invoiceId = null);

    Task<CustomerPayment> GetPaymentByIdAsync(long id);

    Task<bool> AddOrEditAsync(CustomerPayment model);
    Task<InvoiceDetailsDto> GetInvoiceDetails(long invoiceId);
}

public class CustomerPaymentRepository(ApplicationDbContext context) : ICustomerPaymentRepository
{
    private readonly ApplicationDbContext _context = context;

    #region Add Or Edit

    public async Task<bool> AddOrEditAsync(CustomerPayment model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (model.Id == 0)
            {
                await _context.CustomerPayments.AddAsync(model);

                var invoice = await _context.SalesInvoices
                    .FirstOrDefaultAsync(x => x.Id == model.SalesInvoiceId);

                if (invoice == null)
                    throw new Exception("Invoice not found");

                invoice.PaidAmount += model.Amount;
                invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;

                await _context.SaveChangesAsync();

                var lastBalance = await _context.CustomerLedgers
                    .Where(x => x.CustomerId == model.CustomerId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.ClosingBalance)
                    .FirstOrDefaultAsync();

                await _context.CustomerLedgers.AddAsync(new CustomerLedger
                {
                    CustomerId = model.CustomerId,
                    TransactionDate = model.PaymentDate,
                    ReferenceType = "Payment",
                    ReferenceId = model.Id,
                    Description = "Customer Payment",
                    OpeningBalance = lastBalance,
                    Debit = 0,
                    Credit = model.Amount,
                    ClosingBalance = lastBalance - model.Amount
                });
            }
            else
            {
                var payment = await _context.CustomerPayments
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (payment == null)
                    throw new Exception("Payment not found");

                var oldAmount = payment.Amount;

                payment.Amount = model.Amount;
                payment.PaymentDate = model.PaymentDate;
                payment.PaymentMethod = model.PaymentMethod;

                var invoice = await _context.SalesInvoices
                    .FirstOrDefaultAsync(x => x.Id == payment.SalesInvoiceId);

                if (invoice == null)
                    throw new Exception("Invoice not found");

                invoice.PaidAmount =
                    invoice.PaidAmount - oldAmount + model.Amount;

                invoice.DueAmount =
                    invoice.GrandTotal - invoice.PaidAmount;

                var ledger = await _context.CustomerLedgers
                    .FirstOrDefaultAsync(x =>
                        x.ReferenceType == "Payment" &&
                        x.ReferenceId == payment.Id);

                if (ledger != null)
                {
                    ledger.TransactionDate = model.PaymentDate;
                    ledger.Credit = model.Amount;
                    ledger.ClosingBalance =
                        ledger.OpeningBalance - model.Amount;
                }
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion

    #region Payment Details

    public async Task<List<CustomerPayment>> GetAllPaymentsAsync(
        long? customerId = null,
        long? invoiceId = null)
    {
        var query = _context.CustomerPayments
            .Include(x => x.Customer)
            .Include(x => x.SalesInvoice)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId);

        if (invoiceId.HasValue)
            query = query.Where(x => x.SalesInvoiceId == invoiceId);

        return await query
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync();
    }

    #endregion

    #region Ledger History

    public async Task<List<CustomerLedger>> GetCustomerLedgerAsync(
        long? customerId = null,
        long? invoiceId = null)
    {
        var query = _context.CustomerLedgers
            .Include(x => x.Customer)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId);

        if (invoiceId.HasValue)
        {
            query = query.Where(x =>
                x.ReferenceType == "Invoice" ||
                x.ReferenceType == "Payment");
        }

        return await query
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<InvoiceDetailsDto> GetInvoiceDetails(long invoiceId)
    {
        var invoice = await _context.SalesInvoices
            .Where(x => x.Id == invoiceId)
            .Select(x => new
            {
                InvoiceAmount = x.GrandTotal
            })
            .FirstOrDefaultAsync();

        if (invoice == null)
            return null;

        decimal totalPaid = await _context.CustomerPayments
            .Where(x => x.SalesInvoiceId == invoiceId)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        return new InvoiceDetailsDto
        {
            InvoiceAmount = invoice.InvoiceAmount,
            TotalPaid = totalPaid,
            DueAmount = invoice.InvoiceAmount - totalPaid
        };
    }

    #endregion

    #region Get By Id

    public async Task<CustomerPayment> GetPaymentByIdAsync(long id)
    {
        return await _context.CustomerPayments
            .Include(x => x.Customer)
            .Include(x => x.SalesInvoice)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    #endregion
}