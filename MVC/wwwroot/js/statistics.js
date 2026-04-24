onPageLoad();
function onPageLoad() {

    const todaySalesButton = document.getElementById("thisDaySumBtn");
    todaySalesButton.classList.add("active");

    const todayAverageButton = document.getElementById("thisDayAvgBtn").classList.add("active");





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
