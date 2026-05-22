/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.{razor,html,cshtml}",
        "../BizKotei.Client/**/*.{razor,html,cshtml}",
    ],
    darkMode: false,
    theme: {
        extend: {
            fontFamily: {
                sans: [
                    'Hiragino Kaku Gothic Pro',
                    '�q���M�m�p�S Pro W3',
                    'Meiryo',
                    'sans-serif',
                ],
                biz: [
                    'BIZ UDGothic',
                    'Hiragino Kaku Gothic Pro',
                    '�q���M�m�p�S Pro W3',
                    'Meiryo',
                    'sans-serif',
                ],
            },
            height: {
                '100svh': '100svh',
            },
        },
    },
}