function OnChangeCPF(executionContext) {
    var formContext = executionContext.getFormContext();
    var cpf = formContext.data.entity.attributes.getByName('cr1a4_cpf');
    if (cpf.getValue() != null && !validaCpfCnpj(cpf.getValue())) {

        Xrm.Page.ui.setFormNotification('CPF com formato invalido', "ERROR", "ValidaFormatoCPF");
        return false;
    }
    else
        Xrm.Page.ui.clearFormNotification("ValidaFormatoCPF");

    ProcuraContato(cpf.getValue(), executionContext);


}

function ProcuraContato(cpf, executionContext) {
    Xrm.WebApi.retrieveMultipleRecords("contact", "?$select=fullname&$top=3&$filter=cr1a4_cpf eq '" + cpf + "'").then(
        function success(result) {
            if (result.entities.length > 0) {
                Xrm.Page.ui.setFormNotification('CPF Ja existe no cadastro', "ERROR", "PesquisaporCPF");
                var x = Xrm.Page.getControl("cr1a4_cpf");
                x.setFocus();
            }
            else {
                Xrm.Page.ui.clearFormNotification("PesquisaporCPF");
                return true;
            }
        },
        function (error) {
            console.log(error.message);
            // handle error conditions
        }
    );
}

function onSaveCPF(executionContext) {
    var saveEvent = executionContext.getEventArgs();
    var formContext = executionContext.getFormContext();
    var cpf = formContext.data.entity.attributes.getByName('cr1a4_cpf');
    if (cpf.getValue() != null && !validaCpfCnpj(cpf.getValue())) {

        Xrm.Page.ui.setFormNotification('CPF com formato invalido', "ERROR", "ValidaFormatoCPF");
        saveEvent.preventDefault();
    }
    else
        Xrm.Page.ui.clearFormNotification("ValidaFormatoCPF");
}



function OnSaveLead(executionContext) {
    var saveEvent = executionContext.getEventArgs();
    var formContext = executionContext.getFormContext();
    var cpf = formContext.data.entity.attributes.getByName('cr1a4_cpf');
    var cnpj = formContext.data.entity.attributes.getByName('cr1a4_cnpj');
    if (cpf.getValue() == null && cnpj.getValue() == null) {

        Xrm.Page.ui.setFormNotification('Informe ao menos um documento (CPF ou CNPJ)', "ERROR", "ValidaCamposObrigatorios");
        saveEvent.preventDefault();
    }
    else
        Xrm.Page.ui.clearFormNotification("ValidaCamposObrigatorios");
}

function OnChangeCNPJ(executionContext) {
    var formContext = executionContext.getFormContext();
    var cnpj = formContext.data.entity.attributes.getByName('cr1a4_cnpj');
    if (cnpj.getValue() != null && !validaCpfCnpj(cnpj.getValue())) {
        Xrm.Page.ui.setFormNotification('CNPJ com formato invalido', "ERROR", "ValidaFormatoCNPJ")
        return false;

    }
    else
        Xrm.Page.ui.clearFormNotification("ValidaFormatoCNPJ");


    ProcuraConta(cnpj.getValue(), executionContext);
}

function ProcuraConta(cnpj, executionContext) {

    Xrm.WebApi.retrieveMultipleRecords("account", "?$select=name&$top=3&$filter=cr1a4_cnpj eq '" + cnpj + "'").then(
        function success(result) {
            if (result.entities.length > 0) {
                Xrm.Page.ui.setFormNotification('CNPJ Ja existe no cadastro', "ERROR", "PesquisaporCNPJ");
                var x = Xrm.Page.getControl("cr1a4_cnpj");
                x.setFocus();
            }
            else {
                Xrm.Page.ui.clearFormNotification("PesquisaporCNPJ");
                return true;
            }
        },
        function (error) {
            console.log(error.message);
            // handle error conditions
        }
    );



    

}

function onSaveCNPJ(executionContext) {
    var saveEvent = executionContext.getEventArgs();
    var formContext = executionContext.getFormContext();
    var cnpj = formContext.data.entity.attributes.getByName('cr1a4_cnpj');

    if (cnpj.getValue() != null && !validaCpfCnpj(cnpj.getValue())) {

        Xrm.Page.ui.setFormNotification('CNPJ com formato invalido', "ERROR", "ValidaFormatoCNPJ");
        saveEvent.preventDefault();
    }
    else
        Xrm.Page.ui.clearFormNotification("ValidaFormatoCNPJ");
}



function validaCpfCnpj(val) {
    if (val.length == 11) {
        var cpf = val.trim();

        cpf = cpf.split('');

        var v1 = 0;
        var v2 = 0;
        var aux = false;

        for (var i = 1; cpf.length > i; i++) {
            if (cpf[i - 1] != cpf[i]) {
                aux = true;
            }
        }

        if (aux == false) {
            return false;
        }

        for (var i = 0, p = 10; (cpf.length - 2) > i; i++, p--) {
            v1 += cpf[i] * p;
        }

        v1 = ((v1 * 10) % 11);

        if (v1 == 10) {
            v1 = 0;
        }

        if (v1 != cpf[9]) {
            return false;
        }

        for (var i = 0, p = 11; (cpf.length - 1) > i; i++, p--) {
            v2 += cpf[i] * p;
        }

        v2 = ((v2 * 10) % 11);

        if (v2 == 10) {
            v2 = 0;
        }

        if (v2 != cpf[10]) {
            return false;
        } else {
            return true;
        }
    } else if (val.length == 14) {
        var cnpj = val.trim();

        cnpj = cnpj.split('');

        var v1 = 0;
        var v2 = 0;
        var aux = false;

        for (var i = 1; cnpj.length > i; i++) {
            if (cnpj[i - 1] != cnpj[i]) {
                aux = true;
            }
        }

        if (aux == false) {
            return false;
        }

        for (var i = 0, p1 = 5, p2 = 13; (cnpj.length - 2) > i; i++, p1--, p2--) {
            if (p1 >= 2) {
                v1 += cnpj[i] * p1;
            } else {
                v1 += cnpj[i] * p2;
            }
        }

        v1 = (v1 % 11);

        if (v1 < 2) {
            v1 = 0;
        } else {
            v1 = (11 - v1);
        }

        if (v1 != cnpj[12]) {
            return false;
        }

        for (var i = 0, p1 = 6, p2 = 14; (cnpj.length - 1) > i; i++, p1--, p2--) {
            if (p1 >= 2) {
                v2 += cnpj[i] * p1;
            } else {
                v2 += cnpj[i] * p2;
            }
        }

        v2 = (v2 % 11);

        if (v2 < 2) {
            v2 = 0;
        } else {
            v2 = (11 - v2);
        }

        if (v2 != cnpj[13]) {
            return false;
        } else {
            return true;
        }
    } else {
        return false;
    }
}