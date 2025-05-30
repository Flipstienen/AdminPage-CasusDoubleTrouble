function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    const navItems = document.getElementsByClassName("navigation");
    const toggleBtn = document.getElementById("toggleBtn");

    if (sidebar.style.width === "60px") {
        // Expand sidebar
        sidebar.style.width = "250px";
        ProfileSection.style.display = "block";
        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "block";
        }
        toggleBtn.innerHTML = `
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
        `;
    } else {
        // Collapse sidebar
        sidebar.style.width = "60px";
        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "none";
        }
        // Change toggle button icon to a ">" (right arrow)
        toggleBtn.innerHTML = "&gt;";
        toggleBtn.style.fontSize = "24px"; // make it bigger to look good
        toggleBtn.style.lineHeight = "24px";
        ProfileSection.style.display = "none";
    }
}
