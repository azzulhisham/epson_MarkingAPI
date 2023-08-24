(function () {
    $(function () {
        var basicAuthUI =
            '<div class="input"><input placeholder="username" id="input_username" name="username" type="text" size="10"></div>' +
            '<div class="input"><input placeholder="password" id="input_password" name="password" type="password" size="10"></div>' +
            '<div class="input"><a id="custLogin" href="#">Login</a></div>';

        if ($('#input_username').length <= 0) {
            $(basicAuthUI).insertBefore('#api_selector div.input:last-child');
        }

        $("#input_apiKey").hide();
        $("#explore").hide();
        $('#custLogin').click(addAuthorization);

        $('#logo span').text('');
        $('#logo').attr('href', 'https://www.epson.com.my');

        var hostOrigin = window.location.origin;

        $('#logo img').attr('src', hostOrigin + '/epson/images/epsonlogo2.png');
        $('#logo img').attr('height', '35px');
        $('#logo img').attr('width', '100px');
    });

    var saveAccessToken = function (data) {
        $("#custLogin").hide();
        var accessToken = data.access_token;
        var apiKeyAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + accessToken, "header");
        window.swaggerUi.api.clientAuthorizations.add("jsonWebToken", apiKeyAuth);
    };

    function addAuthorization() {
        var username = $('#input_username').val();
        var password = $('#input_password').val();

        if (username.trim() != "" && password.trim() != "") {
            var hostOrigin = window.location.origin;
            var regUrl = hostOrigin + '/Token';
            var data = 'email=' + encodeURIComponent(username) + '&userName=' + encodeURIComponent(username) +
                '&password=' + encodeURIComponent(password);
            data = data + '&grant_type=password';

            $.ajax(regUrl,
                {
                    type: 'POST',
                    data: data,
                    contentType: 'application/json',
                    headers: { 'content-type': 'application/x-www-form-urlencoded' },
                    crossOrigin: true
                })
                .success(saveAccessToken)
                .error(function (jqXHR, textStatus, errorThrown) {
                    //$('#output').text(jqXHR.responseText || textStatus)
                    alert('Bad Request!');
                })
        }
    }
})();