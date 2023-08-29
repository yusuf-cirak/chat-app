/** @type {import('tailwindcss').Config} */
module.exports = {
  purge: {
    enabled: false, // make sure to set this to true for production, false for development
    content: ["./src/**/*.{html,ts}"],
  },
  theme: {
    extend: {
      colors: {
        "wp-blue": "#00a884",
        "wp-gray": "#d1d7db",
        "wp-panel-background": "#F0F2F5",
        "wp-panel-r-border": "#d2d7db",
        "wp-panel-b-border": "#e9edef",
      },
    },
  },
  plugins: [require("@tailwindcss/forms")],
};