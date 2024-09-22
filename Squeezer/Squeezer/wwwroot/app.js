window.copyToClipboard = async (textToCopy) => {
    try {
        await navigator.clipboard.writeText(textToCopy);
    } catch (err) {
        console.error('Error while copying to clipboard', err);
    }
}