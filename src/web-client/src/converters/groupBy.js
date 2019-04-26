export class GroupByValueConverter {
  toView(array, propertyName) {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName) {
      return array;
    }

    let groups = {};

    array.forEach(item => {
      let group = item[propertyName];
      groups[group] = groups[group] || [];
      groups[group].push(item);
    });

    return Object.keys(groups).map(key => {
      let items = groups[key];
      return {
        key: key,
        count: items.length,
        items: items
      };
    });
  }
}
