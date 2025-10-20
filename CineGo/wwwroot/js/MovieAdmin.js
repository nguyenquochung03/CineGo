$('#movieModal').on('shown.bs.modal', function () {
    $(this).find('input:visible:first').focus();
});

function showMovieModal(id) {
    var title = id === 0 ? "Thêm Phim Mới" : "Chỉnh Sửa Phim";
    $("#movieModalLabel").text(title);
    $("#movieFormContainer").html("Đang tải...");

    $.ajax({
        url: '@Url.Action("GetForm", "Movie")',
        type: 'GET',
        data: { id: id },
        success: function (html) {
            $("#movieFormContainer").html(html);
            $('#movieModal').modal('show');
        },
        error: function () {
            $("#movieFormContainer").html('<div class="alert alert-danger">Không thể tải form.</div>');
        }
    });
}

function submitMovieForm(form) {
    var formData = new FormData(form);

    $('.validation-summary-errors').remove();

    $.ajax({
        url: '@Url.Action("Save", "Movie")',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.success) {
                $('#movieModal').modal('hide');
                window.location.reload();
            }
        },
        error: function (xhr) {
            if (xhr.status === 400 || xhr.status === 200) {
                $("#movieFormContainer").html(xhr.responseText);
                $.validator.unobtrusive.parse(form);
            } else {
                alert("Lỗi server: " + xhr.statusText);
            }
        }
    });

    return false;
}

function deleteMovie(id, title) {
    if (confirm(`Bạn có chắc chắn muốn xóa phim "${title}"?`)) {
        $.post('@Url.Action("Delete", "Movie")', { id: id })
            .done(function (result) {
                if (result.success) {
                    window.location.reload();
                } else {
                    alert(result.message || "Xóa thất bại.");
                }
            })
            .fail(function (xhr, status, error) {
                alert("Lỗi server khi xóa.");
            });
    }
}