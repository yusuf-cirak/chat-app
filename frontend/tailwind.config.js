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
      },
    },
  },
  plugins: [require("@tailwindcss/forms")],
};
