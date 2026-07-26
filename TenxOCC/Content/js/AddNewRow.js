document.addEventListener("DOMContentLoaded", function () {
    const addRowBtn = document.querySelector("#addRowBtn");
    const tableBody = document.querySelector("#dataTable tbody");

    function addNewRow() {
        const newRow = document.createElement("tr");
        newRow.classList.add("text-nowrap", "align-middle", "text-center");

        newRow.innerHTML = `
            <td><input type="text" class="inp-bg form-control form-control-sm" placeholder="Item name"/></td>
            <td><input type="text" class="inp-sm form-control form-control-sm" placeholder="Item code"/></td>
            <td><input type="text" class="inp-sm form-control form-control-sm" placeholder="UOM"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" placeholder="0"/></td>
            <td>
                <select class="inp-sm form-select form-select-sm">
                    <option value="Yes">Yes</option>
                    <option value="No">No</option>
                </select>
            </td>
            <td><input type="text" class="inp-sm form-control form-control-sm" placeholder="FOC Remarks"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" placeholder="0.00"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" placeholder="0"/></td>
            <td><input type="text" class="inp-sm form-control form-control-sm" style="min-width:150px;" placeholder="VAT Group"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" style="min-width:120px;" placeholder="0"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" style="min-width:150px;" placeholder="0"/></td>
            <td><input type="text" class="inp-sm form-control form-control-sm" style="min-width:160px;" placeholder="Warehouse"/></td>
            <td><input type="number" class="inp-sm form-control form-control-sm text-end" style="min-width:120px;" placeholder="0"/></td>
        `;

        tableBody.appendChild(newRow);
    }

    // When button clicked → add row
    addRowBtn.addEventListener("click", addNewRow);

    // ✅ Add default row on page load
    addNewRow();
});
