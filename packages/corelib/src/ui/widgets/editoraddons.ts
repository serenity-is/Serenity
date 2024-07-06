import { IconClassName, addClass, iconClassName } from "../../base";
import { registerClass } from "../../base/system";

function ensureInputGroup(editorElement: HTMLElement): HTMLElement {
    let group = editorElement.closest<HTMLElement>('.input-group');

    if (!group) {
        group = document.createElement('div');
        group.className = 'input-group';
        editorElement.insertAdjacentElement('beforebegin', group);
        group.appendChild(editorElement);
    }

    return group;
}

function ensureInputVStack(editorElement: HTMLElement): HTMLElement {
    let group = ensureInputGroup(editorElement);
    let vstack = group.closest<HTMLElement>('.input-vstack');

    if (!vstack) {
        vstack = document.createElement('div');
        vstack.className = 'input-vstack vstack';
        group.insertAdjacentElement('beforebegin', vstack);
        vstack.appendChild(group);
    }

    return vstack;
}

export function inputGroupButtonAddon(props: {
    before?: boolean,
    text: string,
    icon?: IconClassName,
    buttonClass?: string,
    cssClass?: string,
    actionKey?: string,
    editorElement: HTMLElement
}): void {
    const { actionKey, buttonClass, cssClass, icon, text, editorElement, before } = props;

    const group = ensureInputGroup(editorElement);
    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'btn';

    addClass(button, "btn-" + (buttonClass ?? "primary"));
    cssClass && addClass(button, cssClass);
    actionKey && (button.dataset.action = actionKey);

    if (icon) {
        const i = document.createElement("i");
        i.className = iconClassName(icon) + " fa-lg";
        button.append(i);
        text && button.append(" " + text);
    }
    else {
        button.textContent = text ?? "";
    }

    before ? group.prepend(button) : group.append(button);
}

registerClass(inputGroupButtonAddon, 'Serenity.InputGroupButtonAddon');

export function inputGroupTextAddon(props: {
    before?: boolean,
    text: string,
    icon?: IconClassName,
    cssClass?: string,
    editorElement: HTMLElement
}): void {
    const { cssClass, icon, text, editorElement, before } = props;

    const group = ensureInputGroup(editorElement);
    const addon = document.createElement('span');
    addon.className = 'input-group-text';
    cssClass && addClass(addon, cssClass);

    if (icon) {
        const i = document.createElement("i");
        i.className = iconClassName(icon) + " fa-lg";
        addon.append(i);
        text && addon.append(" " + text);
    }
    else {
        addon.textContent = text ?? "";
    }

    before ? group.prepend(addon) : group.append(addon);
}

registerClass(inputGroupTextAddon, 'Serenity.InputGroupTextAddon');

export function formTextAddon(props: {
    text: string,
    before?: boolean,
    cssClass?: string,
    editorElement: HTMLElement
}): void {
    const { cssClass, editorElement, before } = props;

    const vstack = ensureInputVStack(editorElement);
    const addon = document.createElement('div');
    addon.className = 'form-text';
    cssClass && addClass(addon, cssClass);
    addon.textContent = props.text;
    before ? vstack.prepend(addon) : vstack.append(addon);
}

registerClass(formTextAddon, 'Serenity.FormTextAddon');

export { };
