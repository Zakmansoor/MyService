﻿Editservice = (id, name, description, date, active) => {
    document.getElementById("ServiceId").value = id;
    document.getElementById("Name").value = name;
    document.getElementById("Description").value = description;
    // Inside Editservice function
    if (date) {
        document.getElementById("CreatedAt").value = new Date(date).toISOString().split("T")[0];
    } else {
        document.getElementById("CreatedAt").value = "";
    } var Active = document.getElementById("ServiceActive");

    if (active == "True")
        Active.checked = true;
    else
        Active.checked = false;
    document.getElementById("serviceModalLabel").innerHTML = lbTitleEdit;
};

function Rest() {
    document.getElementById("ServiceId").value = "";
    document.getElementById("Name").value = "";
    document.getElementById("Description").value = "";
    document.getElementById("CreatedAt").value = "";
    document.getElementById("ServiceActive").checked = false;
    document.getElementById("serviceModalLabel").innerHTML = lbTitleAdd;
}