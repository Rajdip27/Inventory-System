function showLoader() {
    document.getElementById("loader").style.display = "flex";
}

function hideLoader() {
    document.getElementById("loader").style.display = "none";
}


function confirmDelete(url, message = "Are you sure you want to delete this item?") {
    Swal.fire({
        title: "Confirm Delete",
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}

// <a href="javascript:void(0);"
//     class="btn btn-danger btn-sm"
//     onclick="confirmDelete('/Product/Delete/@item.Id')">
//     Delete
// </a>