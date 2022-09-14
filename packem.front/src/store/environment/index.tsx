const environmentVariable = (key: string) => {
  if (!key) return null;
  return process.env[`REACT_APP_${key.toUpperCase()}`];
};

export { environmentVariable };
