$(function () {

    let index = 0;

    $("#addItem").click(function () {

        addRow(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
    });

    function addRow(
        productId,
        qty,
        pPrice,
        sPrice,
        discount,
        tax,
        transport,
        batch,
        expiry
    ) {

        let productOptions =
            '<option value="">Select Product</option>';

        products.forEach(function (p) {

            let selected =
                productId &&
                    productId.toString() === p.value.toString()
                    ? "selected"
                    : "";

            productOptions += `
                <option value="${p.value}" ${selected}>
                    ${p.text}
                </option>`;
        });

        let row = `
        <div class="item-row border rounded p-3 mb-3">

            <div class="row g-3">

                <div class="col-md-4">
                    <label class="form-label fw-medium">
                        Product
                    </label>

                    <select
                        name="Items[${index}].ProductId"
                        class="toolbar-input">

                        ${productOptions}

                    </select>
                </div>

                <div class="col-md-2">
                    <label class="form-label fw-medium">
                        Quantity
                    </label>

                    <input
                        type="number"
                        name="Items[${index}].Quantity"
                        value="${qty ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Purchase Price
                    </label>

                    <input
                        type="number"
                        step="0.01"
                        name="Items[${index}].PurchasePrice"
                        value="${pPrice ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Sale Price
                    </label>

                    <input
                        type="number"
                        step="0.01"
                        name="Items[${index}].SalePrice"
                        value="${sPrice ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Discount
                    </label>

                    <input
                        type="number"
                        step="0.01"
                        name="Items[${index}].Discount"
                        value="${discount ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Tax
                    </label>

                    <input
                        type="number"
                        step="0.01"
                        name="Items[${index}].TaxAmount"
                        value="${tax ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Transport
                    </label>

                    <input
                        type="number"
                        step="0.01"
                        name="Items[${index}].TransportCost"
                        value="${transport ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Batch No
                    </label>

                    <input
                        type="text"
                        name="Items[${index}].BatchNo"
                        value="${batch ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-medium">
                        Expiry Date
                    </label>

                    <input
                        type="date"
                        name="Items[${index}].ExpiryDate"
                        value="${expiry ?? ''}"
                        class="toolbar-input" />
                </div>

                <div class="col-md-3 d-flex align-items-end">
    <button type="button" class="btn btn-danger btn-sm remove-item">
        <i class="ri-delete-bin-line"></i>
        
    </button>
</div>

            </div>

        </div>`;

        $("#itemsContainer").append(row);

        index++;
    }

    $(document).on(
        "click",
        ".remove-item",
        function () {

            $(this)
                .closest(".item-row")
                .remove();
        });

    if (isEdit && purchaseItems.length > 0) {

        purchaseItems.forEach(function (it) {

            addRow(
                it.productId ?? it.ProductId,
                it.quantity ?? it.Quantity,
                it.purchasePrice ?? it.PurchasePrice,
                it.salePrice ?? it.SalePrice,
                it.discount ?? it.Discount,
                it.taxAmount ?? it.TaxAmount,
                it.transportCost ?? it.TransportCost,
                it.batchNo ?? it.BatchNo,
                (it.expiryDate ?? it.ExpiryDate)
                    ? (it.expiryDate ?? it.ExpiryDate)
                        .split('T')[0]
                    : ""
            );

        });

    } else {

        addRow(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
    }

});