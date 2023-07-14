/** @type {import('tailwindcss').Config} */
module.exports = {
  purge: {
    enabled: false, // make sure to set this to true for production, false for development
    content: ["./src/**/*.{html,ts}"],
  },
  theme: {
    extend: {},
  },
  plugins: [require("@tailwindcss/forms")],
};
