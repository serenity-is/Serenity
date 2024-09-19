if (!process.env.LISTENING_TO_UNHANDLED_REJECTION) {
    process.on('unhandledRejection', err => {
        try {
            if (!err || !err.reason)
                return;

            const reason = err.reason;
            if (reason.origin == "serviceCall" ||
                reason.origin == "test") {
                err.preventDefault();

                if (!reason.silent &&
                    (reason.kind ?? "exception") === "exception") {
                    console.error(err);
                }
            }
        }
        catch {
        }
    })
    // Avoid memory leak by adding too many listeners
    process.env.LISTENING_TO_UNHANDLED_REJECTION = true
  }