export class FilterByValueConverter {
  toView(array, propertyName, text) {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName) {
      return array;
    }

    if (!text) {
      return array;
    }

    text = text.toLocaleLowerCase();

    return array.filter(item => {
      let value = item[propertyName];
      if (!value) return;
      return value.toLocaleLowerCase().indexOf(text) !== -1;
    });
  }
}
