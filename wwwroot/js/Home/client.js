let isEditing = false; // 記錄是否處於編輯狀態
// 個人頁切編輯
function showEditPage() {
    isEditing = true;
    $('#tab1').hide();
    $('#editPage').show();
}
// 個人頁切檢視
function showTab1Page() {
    isEditing = false;
    $('#editPage').hide();
    $('#tab1').show();
}
// 儲存按鈕
function saveEdit(e) {
    e.preventDefault();
    alert('變更已儲存');
    showTab1Page();
}
// 取消編輯按鈕
function cancelEdit() {
    if (confirm('確定要取消編輯？未儲存資料將會遺失！')) {
        showTab1Page();
    }
}
// 側邊欄點擊 → 防呆檢查
$('#sidebarTabs .nav-link').on('show.bs.tab', function (e) {
    if (isEditing) {
        let confirmed = confirm('尚未儲存變更，是否要離開？');
        if (confirmed) {
            isEditing = false;
            showTab1Page(); // 切回檢視頁面
        } else {
            e.preventDefault(); // 阻止 tab 切換 → 點取消時畫面保持不變
        }
    }
});

function switchToTab(tabId) {
    const trigger = document.querySelector(`[data-bs-target="#${tabId}"]`);
    if (trigger) {
        const tab = new bootstrap.Tab(trigger);
        tab.show();
    } else {
        console.warn(`找不到 data-bs-target="#${tabId}" 的 tab 控制元件`);
    }
}

//首頁編輯
$("#editPasswordBtn").on("click", () => {
    $("#editPasswordBtn").addClass("d-none")
    $("#confirmPasswordBtn").removeClass("d-none")
    $("#cancelPasswordBtn").removeClass("d-none")

    $("#passwordDisplay").addClass("d-none")
    $("#passwordInput1").removeClass("d-none")
})

$("#confirmPasswordBtn").on("click", () => {
    if ($("#passwordInput1").val() != "") {
        alert("變更未確認，再次輸入密碼")

        $("#confirmPasswordBtn").addClass("d-none")
        $("#savePasswordBtn").removeClass("d-none")

        $("#passwordInput1").addClass("d-none")
        $("#passwordInput2").removeClass("d-none")
    }
})

$("#savePasswordBtn").on("click", () => {
    if ($("#passwordInput2").val() != "") {
        if ($("#passwordInput1").val() != $("#passwordInput2").val()) {
            alert("密碼錯誤，再次輸入密碼")

            $("#passwordInput2").val("")
        }
        else {
            alert("變更已儲存")

            $("#passwordInput3").val($("#passwordInput2").val())
            $("#btn_password").trigger("click")

            $("#editPasswordBtn").removeClass("d-none")
            $("#savePasswordBtn").addClass("d-none")
            $("#cancelPasswordBtn").addClass("d-none")

            $("#passwordDisplay").removeClass("d-none")
            $("#passwordInput2").addClass("d-none")
        }
    }
})

$("#cancelPasswordBtn").on("click", () => {
    if ($("#passwordInput1").val() != "" || $("#passwordInput2").val() != "") { alert("變更未儲存，確定取消?") }

    $("#editPasswordBtn").removeClass("d-none")
    $("#confirmPasswordBtn").addClass("d-none")
    $("#savePasswordBtn").addClass("d-none")
    $("#cancelPasswordBtn").addClass("d-none")

    $("#passwordDisplay").removeClass("d-none")
    $("#passwordInput1").addClass("d-none").val("")
    $("#passwordInput2").addClass("d-none").val("")
})

$("#editPhoneBtn").on("click", () => {
    $("#editPhoneBtn").addClass("d-none")
    $("#savePhoneBtn").removeClass("d-none")
    $("#cancelPhoneBtn").removeClass("d-none")

    $("#phoneDisplay").addClass("d-none")
    $("#phoneInput1").removeClass("d-none")
})

$("#savePhoneBtn").on("click", () => {
    if ($("#phoneInput1").val() != $("#phoneDisplay").text()) { alert("變更已儲存") }

    $("#phoneInput2").val($("#phoneInput1").val())
    $("#btn_phone").trigger("click")

    $("#editPhoneBtn").removeClass("d-none")
    $("#savePhoneBtn").addClass("d-none")
    $("#cancelPhoneBtn").addClass("d-none")

    $("#phoneDisplay").removeClass("d-none").text($("#phoneInput1").val())
    $("#phoneInput1").addClass("d-none").val($("#phoneDisplay").text())
})

$("#cancelPhoneBtn").on("click", () => {
    if ($("#phoneInput1").val() != $("#phoneDisplay").text()) { alert("變更未儲存，確定取消?") }

    $("#editPhoneBtn").removeClass("d-none")
    $("#savePhoneBtn").addClass("d-none")
    $("#cancelPhoneBtn").addClass("d-none")

    $("#phoneDisplay").removeClass("d-none")
    $("#phoneInput1").addClass("d-none").val($("#phoneDisplay").text())
})

$(".btn_cFav").on("click", function () {
    let collectionId = $(this).closest("tr").find(".td_Fav").text().trim();

    alert("變更已儲存")

    $("#collectionIdInput1").val(collectionId)
    $("#btn_cFavS").trigger("click")
})

$(".btn_dFav").on("click", function () {
    let collectionId = $(this).closest("tr").find(".td_Fav").text().trim();

    alert("變更已儲存")

    $("#collectionIdInput2").val(collectionId)
    $("#btn_dFavS").trigger("click")
})

//訊息頁刪除功能
function deleteRow(button) {
    const row = button.closest("tr");
    if (row) {
        row.remove();
    }
}

$("#btn-tab5").on("click", () => { $("#btn_logout").trigger("click") })