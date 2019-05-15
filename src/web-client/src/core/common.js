const getMonths = () => {
  return [
    'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
  ];
};

const getDays = () => {
  return ([...Array(31).keys()].map(day => {
    let item = day + 1;
    item = item >= 10 ? item : `0${item}`;
    return item.toString();
  }));
};

const getYears = () => {
  return ([...Array(100).keys()].reverse().map(year => {
    let item = year + 1920;
    return item.toString();
  }));
};

export { getMonths, getDays, getYears };
