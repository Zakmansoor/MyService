Editservice = (id, name,arname, description, date, active) => {
    document.getElementById("ServiceId").value = id;
    document.getElementById("Name").value = name;
    document.getElementById("Namear").value = arname;
    document.getElementById("Description").value = description;
    // Inside Editservice function
    if (date) {
        document.getElementById("Createdat").value = new Date(date).toISOString().split("T")[0];
    } else {
        document.getElementById("Createdat").value = "";
    } var Active = document.getElementById("Serviceactive");

    if (active == "True")
        Active.checked = true;
    else
        Active.checked = false;
    document.getElementById("serviceModalLabel").innerHTML = lbTitleEdit;
};

function Rest() {
    document.getElementById("ServiceId").value = "";
    document.getElementById("Name").value = "";
    document.getElementById("Namear").value = "";
    document.getElementById("Description").value = "";
    document.getElementById("Createdat").value = "";
    document.getElementById("Serviceactive").checked = false;
    document.getElementById("serviceModalLabel").innerHTML = lbTitleAdd;
}