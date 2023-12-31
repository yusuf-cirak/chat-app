/** @type {import('tailwindcss').Config} */
module.exports = {
  purge: {
    enabled: false, // make sure to set this to true for production, false for development
    content: ["./src/**/*.{html,ts}"],
  },
  theme: {
    extend: {
      colors: {
        "wp-green": "#00a884",
        "wp-gray": "#d1d7db",
        "wp-panel-background": "#F0F2F5",
        "wp-panel-r-border": "#d2d7db",
        "wp-panel-b-border": "#e9edef",
        "wp-my-message-background": "#dcfcd4",
        "wp-message-date-color": "#6d6c6b",
        "wp-panel-active-bg": "#d9dbdf",
        "wp-badge-text": "#54656f",
        "wp-hover-gray-bg": "#3b4a54",
        "wp-unread-message-badge": "#4ad26c",
      },
    },
  },
  plugins: [require("@tailwindcss/forms")],
};
