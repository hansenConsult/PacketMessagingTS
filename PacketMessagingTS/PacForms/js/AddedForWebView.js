

function SetMessageNumber(messageNumber, callsign, name) {
    document.forms[0].msgno.value = messageNumber;
    document.forms[0].ocall.value = callsign;
    document.forms[0].oname.value = name;
}

function LoadInlArray1(startIndex, p1) {
    var i = Number(startIndex);
    inl[i] = p1;
}

function LoadInlArray2(startIndex, p1, p2) {
    var i = Number(startIndex);
    inl[i] = p1;
    inl[i + 1] = p2;
}

function LoadInlArray3(startIndex, p1, p2, p3) {
    var i = Number(startIndex);
    inl[i] = p1;
    inl[i + 1] = p2;
    inl[i + 2] = p3;
}

function LoadInlArray4(startIndex, p1, p2, p3, p4) {
    var i = Number(startIndex);
    inl[i] = p1;
    inl[i + 1] = p2;
    inl[i + 2] = p3;
    inl[i + 3] = p4;
}

function LoadInlArray5(startIndex, p1, p2, p3, p4, p5) {
    var i = Number(startIndex);
    inl[i] = p1;
    inl[i + 1] = p2;
    inl[i + 2] = p3;
    inl[i + 3] = p4;
    inl[i + 4] = p5;
}

function LoadInrArray1(startIndex, p1) {
    var i = Number(startIndex);
    inr[i] = p1;
}

function LoadInrArray2(startIndex, p1, p2) {
    var i = Number(startIndex);
    inr[i] = p1;
    inr[i + 1] = p2;
}

function LoadInrArray3(startIndex, p1, p2, p3) {
    var i = Number(startIndex);
    inr[i] = p1;
    inr[i + 1] = p2;
    inr[i + 2] = p3;
}

function LoadInrArray4(startIndex, p1, p2, p3, p4) {
    var i = Number(startIndex);
    inr[i] = p1;
    inr[i + 1] = p2;
    inr[i + 2] = p3;
    inr[i + 3] = p4;
}

function LoadInrArray5(startIndex, p1, p2, p3, p4, p5) {
    var i = Number(startIndex);
    inr[i] = p1;
    inr[i + 1] = p2;
    inr[i + 2] = p3;
    inr[i + 3] = p4;
    inr[i + 4] = p5;
}

function PopulateForm(val) {
    outels();
}

function SubmitForm() {
    newwin(1);

    window.external.notify(ascii);
}
