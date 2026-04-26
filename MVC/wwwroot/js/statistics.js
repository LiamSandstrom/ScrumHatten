document.addEventListener('DOMContentLoaded', onPageLoad);

async function onPageLoad() {
    const todaySalesButton = document.getElementById("thisDaySumBtn");
    if (todaySalesButton) todaySalesButton.classList.add("active");

    const todayAverageButton = document.getElementById("thisDayAvgBtn");
    if (todayAverageButton) todayAverageButton.classList.add("active");

    try {
        await loadSalesLineChart();
    } catch (err) {
        console.error('Failed to load sales chart:', err);
    }
}

function showSalesToday() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }

    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }


    const salesElement = document.getElementById("thisDaySum").classList.remove("noDisplay");
    const hatElement = document.getElementById("thisDayHatSum").classList.remove("noDisplay");

}

function showSalesPastDay() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }

    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }

    const salesElement = document.getElementById("pastDaySum").classList.remove("noDisplay");
    const hatElement = document.getElementById("pastDayHatSum").classList.remove("noDisplay")


}

function showSalesPastWeek() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }

    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }

    const salesElement = document.getElementById("pastWeekSum").classList.remove("noDisplay");
    const hatElement = document.getElementById("pastWeekHatSum").classList.remove("noDisplay")


}

function showSalesPastMonth() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }
    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }

    const salesElement = document.getElementById("pastMonthSum").classList.remove("noDisplay");
    const hatElement = document.getElementById("pastMonthHatSum").classList.remove("noDisplay")



}

function showSalesPastYear() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }
    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }


    const salesElement = document.getElementById("pastYearSum").classList.remove("noDisplay");
    const hatElement = document.getElementById("pastYearHatSum").classList.remove("noDisplay")


}

function showSalesThisYear() {

    const allOrdersumElements = document.getElementsByClassName("orderSum");

    for (let i = 0; i < allOrdersumElements.length; i++) {
        allOrdersumElements[i].classList.add("noDisplay");
    }
    const allHatAmountElements = document.getElementsByClassName("hatSum");

    for (let i = 0; i < allHatAmountElements.length; i++) {
        allHatAmountElements[i].classList.add("noDisplay");
    }

    const salesElement = document.getElementById("thisYearSum").classList.remove("noDisplay");
    const hatElement = document.getElementById("thisYearHatSum").classList.remove("noDisplay")


}

function showAvgSalesToday() {

    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++)
    {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("thisDayAvg").classList.remove("noDisplay");

}

function showAvgSalesPastDay() {

    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++) {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("pastDayAvg").classList.remove("noDisplay");

}

function showAvgSalesPastWeek() {
    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++) {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("pastWeekAvg").classList.remove("noDisplay");

}

function showAvgSalesPastMonth() {
    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++) {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("pastMonthAvg").classList.remove("noDisplay");

}

function showAvgSalesPastYear() {

    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++) {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("pastYearAvg").classList.remove("noDisplay");

}

function showAvgSalesThisYear() {

    const allOrderAvgElements = document.getElementsByClassName("orderAvg")

    for (let i = 0; i < allOrderAvgElements.length; i++) {
        allOrderAvgElements[i].classList.add("noDisplay");
    }

    const avgElement = document.getElementById("thisYearAvg").classList.remove("noDisplay");

}

async function loadSalesData() {
    const url = '/Statistics/SalesData?monthsBack=12';
    const response = await fetch(url, { headers: { 'Accept': 'application/json' } });

    const text = await response.text();

    if (!response.ok) {
        console.error(`Fetch failed (${response.status}):`, text);
        throw new Error(`Request failed with status ${response.status}`);
    }

    // Try parse JSON and log raw text if it fails
    try {
        return JSON.parse(text);
    } catch (e) {
        console.error('Response is not valid JSON. Response text:', text);
        throw e;
    }
}

async function loadSalesLineChart() {
    const fetchedData = await loadSalesData();

    if (!Array.isArray(fetchedData)) {
        console.error('Expected array from API, got:', fetchedData);
        return;
    }

    const labels = [];
    const salesData = [];

    for (let i = 0; i < fetchedData.length; i++) {
        const obj = fetchedData[i];
        labels.push(obj.monthName);
        salesData.push(obj.totalsales);
    }

    const canvas = document.getElementById('salesChart');
    if (!canvas) {
        console.error('Canvas element with id "salesChart" not found.');
        return;
    }
    const ctx = canvas.getContext ? canvas.getContext('2d') : canvas;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Omsättning',
                data: salesData,
                borderWidth: 3,
                tension: 0.2,
                fill: false,
                pointRadius: 3
            }]
        },
        options: {

            maintainAspectRatio: false,

            scales: {

                x: {
                    grid: {
                        drawOnChartArea: false
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        drawOnChartArea: false
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
}
