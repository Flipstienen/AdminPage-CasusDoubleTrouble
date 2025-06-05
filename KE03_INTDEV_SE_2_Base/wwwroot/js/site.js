function toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    const navItems = document.getElementsByClassName("navigation");
    const toggleBtn = document.getElementById("toggleBtn");
    const ProfileSection = document.getElementById("ProfileSection");
    const navImages = sidebar.querySelectorAll("ul.nav img");

    const isCollapsed = sidebar.style.width === "60px";

    if (isCollapsed) {
        sidebar.style.width = "250px";
        ProfileSection.style.display = "block";

        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "block";
        }

        for (let img of navImages) {
            img.classList.add("me-2");
            img.style.width = "24px";
        }

        sidebar.querySelectorAll('a.quickselectsidebar.nav-link').forEach(link => {
            link.style.padding = '';
            link.classList.remove("mt-2")
        });

        toggleBtn.innerHTML = `
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
            <span style="display:block; width: 22px; height: 2px; background-color: black; margin: 4px 0;"></span>
        `;
        toggleBtn.style.fontSize = "";
        toggleBtn.style.lineHeight = "";

    } else {
        sidebar.style.width = "60px";
        ProfileSection.style.display = "block";

        for (let i = 0; i < navItems.length; i++) {
            navItems[i].style.display = "none";
        }

        for (let img of navImages) {
            img.classList.remove("me-2");
            img.style.width = "30px";
        }

        sidebar.querySelectorAll('a.quickselectsidebar.nav-link').forEach(link => {
            link.style.padding = '0';
            link.classList.add("mt-2")
        });

        toggleBtn.innerHTML = "&gt;";
        toggleBtn.style.fontSize = "24px";
        toggleBtn.style.lineHeight = "24px";
        ProfileSection.style.display = "none";
    }
}
