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

const getUrlParameter = (name, url) => {
  if (!url) url = window.location.href;
  name = name.replace(/[[\]]/g, '\\$&');
  let regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
      results = regex.exec(url);
  if (!results) return null;
  if (!results[2]) return '';
  return decodeURIComponent(results[2].replace(/\+/g, ' '));
};

export { getMonths, getDays, getYears, getUrlParameter };
