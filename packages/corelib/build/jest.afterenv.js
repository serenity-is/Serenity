if (Object["__definePropertyMocked__"] !== true) {
    Object["__definePropertyMocked__"] = true;
    const originalDefineProperty = Object.defineProperty;
    const mutableDefineProperty = (obj, prop, attributes) => {
        // this is to prevent the error `Cannot redefine property: prototype`; prototype can not be configurable...
        if (prop === "prototype") return originalDefineProperty(obj, prop, attributes);
        return originalDefineProperty(
            obj,
            prop,
            {
                configurable: true,
                ...attributes
            }
        );
    }
    Object.defineProperty = mutableDefineProperty;
}
