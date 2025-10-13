import { className } from "./classname"
import type { BasicClassList, ClassNames } from "./types"
import type { SignalLike } from "./util"

export function useClassList(initialValue?: ClassNames) {
  const div = document.createElement("div")
  if (initialValue != null) {
    div.className = className(initialValue)
  }

  let list = div.classList

  function ClassList(value: Element) {
    value.setAttribute("class", list.value)
    list = value.classList
  }

  Object.defineProperties(
    ClassList,
    Object.getOwnPropertyDescriptors({
      get size() {
        return list.length
      },
      get value() {
        return list.value
      },
      add(...tokens: string[]) {
        list.add(...tokens)
      },
      remove(...tokens: string[]) {
        list.remove(...tokens)
      },
      toggle(token: string, force?: boolean) {
        list.toggle(token, force)
      },
      contains(token: string) {
        return list.contains(token)
      },
    })
  )

  return ClassList as BasicClassList
}

export function useText(initialValue?: string) {
  const text = new Text()
  Object.defineProperty(text, "toString", {
    value() {
      return this.textContent
    },
  })
  function setText(value: string) {
    text.textContent = value
  }
  if (initialValue != null) {
    setText(initialValue)
  }
  return [text, setText] as const
}
