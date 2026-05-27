window.maskFunctions = {

    apply: (elementId, maskType) => {

        const element =
            document.getElementById(elementId);

        if (!element)
            return;

        switch (maskType) {

            case "Phone":

                IMask(element, {
                    mask: '(00)00000-0000'
                });

                break;

            case "CPF":

                IMask(element, {
                    mask: '000.000.000-00'
                });

                break;

            case "CNPJ":

                IMask(element, {
                    mask: '00.000.000/0000-00'
                });

                break;

            case "Time":

                IMask(element, {
                    mask: '00:00'
                });

                break;

            case "Date":

                IMask(element, {
                    mask: '00/00/0000'
                });

                break;
        }
    }
};