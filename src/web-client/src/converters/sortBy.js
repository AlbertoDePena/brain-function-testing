export class SortByValueConverter {
  toView(array, propertyName, direction) {
    if (!array || array.length === 0) {
      return [];
    }

    if (!propertyName) {
      return array;
    }

    let factor = direction === 'asc' ? 1 : -1;

		return array.sort((a, b) => {
			let comparison = 0;
			if (a[propertyName] > b[propertyName]) {
				comparison = 1;
			} else if (a[propertyName] < b[propertyName]) {
				comparison = -1;
			}
			return comparison * factor;
		});
  }
}
