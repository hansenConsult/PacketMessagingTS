

function SetMessageNumber(messageNumber, callsign, name) {
    document.forms[0].msgno.value = messageNumber;
    document.forms[0].ocall.value = callsign;
    document.forms[0].oname.value = name;
}

function LoadInrArray(InrArray) {
    //for (i = 0; i < InrArray.length; i++) {
    for (i = 0; i < 1; i++) {
        inr[i] = InrArray[i];
    }
}

function LoadInlArray(InlArray) {
    //for (i = 0; i < InlArray.length; i++) {
    for (i = 0; i < 1; i++) {
        inl[i] = Number(InlArray[i]);
    }
}

function PopulateForm(val) {
    outels();
}
