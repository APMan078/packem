export const isUsername = (value) => {
  if (!value) return false;
  const result = value.toString();
  const usernameRegex = /^[a-z0-9/]+$/;
  return usernameRegex.test(result);
};
