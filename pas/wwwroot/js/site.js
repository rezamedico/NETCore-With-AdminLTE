// Write your JavaScript code.
function notification(_type, _message, _delay) {
    if (_type == "success") {
        $.notify({
            // options
            icon: 'glyphicon glyphicon-ok',
            title: 'Berhasil!',
            message: _message
        }, {
                // settings
                type: 'success',
                delay: _delay
            });
    } else if (_type == "danger") {

        $.notify({
            // options
            icon: 'glyphicon glyphicon-remove',
            title: 'Gagal!',
            message: _message
        }, {
                // settings
                type: 'danger',
                delay: _delay
            });

    }
}