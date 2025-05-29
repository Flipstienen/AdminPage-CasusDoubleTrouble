function toggleSidebar() {
    const navItems = document.getElementsByClassName("navigation");
    document.getElementById("sidebar").classList.toggle("collapsed");
    if (navItems[0].style.display === "none") {
        //document.getElementById("closed").style.display = "none"

        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "block";
        }
    }

    else {
        //document.getElementById("closed").style.display = "block"

        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "none"
        }
    }
}