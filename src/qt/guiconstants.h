#ifndef GUICONSTANTS_H
#define GUICONSTANTS_H

/* Milliseconds between model updates */
static const int MODEL_UPDATE_DELAY = 250;

/* AskPassphraseDialog -- Maximum passphrase length */
static const int MAX_PASSPHRASE_SIZE = 1024;

/* BitcoinGUI -- Size of icons in status bar */
static const int STATUSBAR_ICONSIZE = 16;

/* Invalid field background style FF 80 80       Gray*/
#define STYLE_INVALID "background:#00FFFF"

/* Transaction list -- unconfirmed transaction  (Very -GREEN) */
#define COLOR_UNCONFIRMED QColor(0, 255,55)
/* Transaction list -- negative amount (Red) */
#define COLOR_NEGATIVE QColor(200, 0, 0)

/* Transaction list -- bare address (without label) */
#define COLOR_BAREADDRESS QColor(0, 240, 240)

/* Tooltips longer than this (in characters) are converted into rich text,
   so that they can be word-wrapped.
 */
static const int TOOLTIP_WRAP_THRESHOLD = 80;

/* Maximum allowed URI length */
static const int MAX_URI_LENGTH = 255;

/* QRCodeDialog -- size of exported QR Code image */
#define EXPORT_IMAGE_SIZE 256

#endif // GUICONSTANTS_H
