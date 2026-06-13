$(function () {

    let index = 0;

    // =========================
    // ADD ROW BUTTON
    // =========================
    $("#addItem").click(function () {
        addRow("", "", "", "", "", "", "", "", "");
    });

    // =========================
    // CREATE ROW
    // =========================
    function addRow(productId, qty, pPrice, sPrice, discount, tax, transport, batch, expiry) {

        let row = `
        <div class="row g-3 item-row mb-3">

            <div class="col-md-2">
                <label class="form-label">Product Id</label>
                <input name="Items[${index}].ProductId" value="${productId}" class="toolbar-input" />
            </div>

            <div class="col-md-1">
                <label class="form-label">Qty</label>
                <input name="Items[${index}].Quantity" value="${qty}" class="toolbar-input" />
            </div>

            <div class="col-md-2">
                <label class="form-label">Purchase Price</label>
                <input name="Items[${index}].PurchasePrice" value="${pPrice}" class="toolbar-input" />
            </div>

            <div class="col-md-2">
                <label class="form-label">Sale Price</label>
                <input name="Items[${index}].SalePrice" value="${sPrice}" class="toolbar-input" />
            </div>

            <div class="col-md-1">
                <label class="form-label">Discount</label>
                <input name="Items[${index}].Discount" value="${discount}" class="toolbar-input" />
            </div>

            <div class="col-md-1">
                <label class="form-label">Tax</label>
                <input name="Items[${index}].TaxAmount" value="${tax}" class="toolbar-input" />
            </div>

            <div class="col-md-1">
                <label class="form-label">Transport</label>
                <input name="Items[${index}].TransportCost" value="${transport}" class="toolbar-input" />
            </div>

            <div class="col-md-2">
                <label class="form-label">Batch No</label>
                <input name="Items[${index}].BatchNo" value="${batch}" class="toolbar-input" />
            </div>

            <div class="col-md-2">
                <label class="form-label">Expiry</label>
                <input type="date" name="Items[${index}].ExpiryDate" value="${expiry}" class="toolbar-input" />
            </div>

            <div class="col-md-1 d-flex align-items-end">
                <button type="button" class="btn btn-danger btn-sm remove-item">
                    X
                </button>
            </div>

        </div>`;

        $("#itemsContainer").append(row);
        index++;
    }

    // =========================
    // REMOVE ROW
    // =========================
    $(document).on("click", ".remove-item", function () {
        $(this).closest(".item-row").remove();
    });

    // =========================
    // EDIT MODE LOAD
    // =========================
    let isEdit = $("#IsEdit").val() === "true";

    if (isEdit) {

        let itemsJson = $("#ItemsJson").val();

        if (itemsJson) {

            let items = JSON.parse(itemsJson);

            for (let i = 0; i < items.length; i++) {

                let it = items[i];

                addRow(
                    it.ProductId,
                    it.Quantity,
                    it.PurchasePrice,
                    it.SalePrice,
                    it.Discount,
                    it.TaxAmount,
                    it.TransportCost,
                    it.BatchNo,
                    it.ExpiryDate ? it.ExpiryDate.split('T')[0] : ""
                );
            }
        }
    }

});