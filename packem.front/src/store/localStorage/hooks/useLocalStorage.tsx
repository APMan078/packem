import React from 'react';

function useLocalStorage(key, defaultValue) {
  const [value, setValue] = React.useState(() => {
    let storageValue = window.localStorage.getItem(key);
    storageValue = storageValue === 'null' ? null : storageValue;
    return storageValue !== null ? JSON.parse(storageValue) : defaultValue;
  });
  React.useEffect(() => {
    const newValue =
      value != null && value !== 'null' ? JSON.stringify(value) : null;
    window.localStorage.setItem(key, newValue);
  }, [key, value]);
  return [value, setValue];
}

export default useLocalStorage;
