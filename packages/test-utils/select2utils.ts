export function findActiveSelect2Dropdown() {
    const select2Drop = document.querySelector<HTMLElement>(`.select2-drop.select2-drop-active:not(.select2-display-none)`);
    return { select2Drop, select2Results: select2Drop ? select2Drop.querySelectorAll<HTMLElement>(".select2-result") ?? [] : [] };
}

export function clickSelect2Result(select2Result: HTMLElement) {
    if (!select2Result.classList.contains("select2-highlighted")) {
        select2Result.parentNode?.querySelectorAll<HTMLElement>(".select2-result").forEach(x => x.classList.remove("select2-highlighted"));
        select2Result.classList.add("select2-highlighted");
    }
    select2Result.dispatchEvent(new MouseEvent("mouseup", { bubbles: true })); // select 2nd customer (Berglunds snabbköp)
}

