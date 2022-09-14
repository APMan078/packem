import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { isUsername } from 'helpers/validators';
import {
  createUser,
  editUser,
  getUserVendor,
} from 'services/api/user/users.api';
import { getVendorLookup } from 'services/api/vendors/vendors.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import {
  Modal,
  Box,
  Typography,
  Autocomplete,
  TextField,
  Chip,
} from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface CustomerUserModalProps {
  userData?: any;
  admin?: boolean;
  superAdmin?: boolean;
  edit?: boolean;
  callBack?: () => void;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

export default React.memo(
  ({ userData, admin, superAdmin, callBack, edit }: CustomerUserModalProps) => {
    const theme = useTheme();
    const defaultAutocompleteOption: AutoCompleteOptionType | null = {
      id: -1,
      label: '',
    };
    const userRoles = [
      { id: 2, label: 'Admin' },
      { id: 3, label: 'Ops Manager' },
      { id: 4, label: 'Operator' },
      { id: 5, label: 'Viewer' },
    ];
    const [viewerSelected, setViewerSelected] = useState(false);
    const [vendors, setVendors] = useState<any>([]);
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const { isUserModalOpen, onCloseUserModal, handleUpdateData, updateData } =
      useContext(GlobalContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      role: '',
      password: '',
    };
    const initialState: any = {
      userId: '',
      customerLocationId: '',
      customerId: '',
      name: '',
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      role: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      password: '',
    };
    const [formErrors, setFormErrors] = useState(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialState);
    const [viewerVendors, setViewerVendors] = useState<any>([]);
    const [selectedVendor, setSelectedVendor] =
      useState<AutoCompleteOptionType | null>(defaultAutocompleteOption);

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const onLoadCustomerVendors = async (customerId: number) => {
      try {
        const vendorsFromApi = await getVendorLookup(customerId, '');
        setVendors(vendorsFromApi);
        return true;
      } catch (err: any) {
        return err;
      }
    };

    const onLoadViewerVendors = async (userId) => {
      try {
        const viewerVendorsFromApi = await getUserVendor(userId);

        setViewerVendors(viewerVendorsFromApi);

        return true;
      } catch (err: any) {
        return err;
      }
    };

    const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
      onForm(key, event.target.value);

    const autoCompleteInputHandler = (
      key: string,
      value: AutoCompleteOptionType,
    ) => {
      if (value.id === 5) {
        setViewerSelected(true);
        if (superAdmin) {
          onLoadCustomerVendors(userData.customerId);
        } else {
          onLoadCustomerVendors(currentUser.Claim_CustomerId);
        }
      } else {
        setViewerSelected(false);
      }

      setForm(() => ({
        ...form,
        [key]: value,
      }));
    };

    const vendorInputHandler = (key: string, value: AutoCompleteOptionType) => {
      if (value !== null) {
        setSelectedVendor(value);
        const exists = viewerVendors.find(
          (vendor) => vendor.vendorId.toString() === value.id.toString(),
        );

        if (!exists) {
          setViewerVendors([
            ...viewerVendors,
            vendors.find((vendor) => vendor.vendorId === value.id),
          ]);
        }
      }
    };

    const schema = yup.object().shape({
      customerLocationId: yup.number().required('Required.'),
      customerId: yup.number().required('Required.'),
      firstName: yup.string().required('Required.'),
      lastName: yup.string().required('Required.'),
      name: yup.string().required('Required.'),
      username: yup
        .string()
        .required('Required')
        .test(
          'is-valid-username',
          'Incorrect username format. Ex.] ajm28',
          (value) => isUsername(value),
        ),
      email: yup.string().required('Required'),
      role: yup
        .object()
        .shape({ id: yup.string().nullable(), label: yup.string() })
        .test(
          'empty-check',
          'A role must be selected',
          (vendor) => !!vendor.label,
        )
        .typeError('Required.'),
      password: yup.string().required('Required'),
    });

    const vendorSelectionOptions = Array.isArray(vendors)
      ? vendors.map((vendor, index) => ({
          id: vendor.vendorId,
          label: `${vendor.name}`,
        }))
      : [{ id: 0, label: 'Vendor' }];

    const handleCloseUserModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      if (edit) {
        callBack();
      }
      setViewerSelected(false);
      setSelectedVendor({
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      });
      setViewerVendors([]);
      onCloseUserModal();
    };

    const onSaveNewUser = async () => {
      if (superAdmin) {
        form.customerId = userData.customerId;
        form.customerLocationId = userData.customerLocationId;
      } else {
        form.customerId = currentUser.Claim_CustomerId;
        form.customerLocationId = currentLocationAndFacility.customerLocationId;
      }
      form.name = `${form.firstName} ${form.lastName}`;
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            const newForm: any = form;
            newForm.role = form.role.id;
            newForm.vendorIds = viewerVendors.map((vendor) => vendor.vendorId);
            if (!edit) {
              await createUser(newForm);
              if (superAdmin) {
                snackActions.success(
                  `Successfully created new user for ${userData.name}.`,
                );
              } else {
                snackActions.success(
                  `Successfully created new user at ${currentLocationAndFacility.locationName}.`,
                );
              }
            } else {
              newForm.userId = userData.userId;
              await editUser(userData.userId, newForm);
              if (superAdmin) {
                snackActions.success(
                  `Successfully edited user: ${userData.username} for ${userData.name}.`,
                );
              } else {
                snackActions.success(
                  `Successfully edited user: ${userData.username} at ${currentLocationAndFacility.locationName}.`,
                );
              }
            }
            handleCloseUserModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors(initialFormErrorsState);
            setShowErrors(true);
            snackActions.error(`${err}`);
          }
        })
        .catch((err) => {
          const errorsFound = err.inner.reduce((acc, curr) => {
            if (!acc[curr.path]) acc[curr.path] = curr.message;
            return acc;
          }, {});
          setFormErrors(errorsFound);
          setError('');
          setShowErrors(true);
        });
    };

    const handleSetEditForm = () => {
      setForm({
        customerLocationId: userData.customerLocationId,
        customerId: userData.customerId,
        name: userData.name,
        firstName: userData.name.split(' ')[0],
        lastName: userData.name.split(' ')[1],
        username: userData.username,
        email: userData.email,
        role: {
          id: userData.roleId,
          label: userRoles.filter((r) => r.id === userData.roleId)[0].label,
        },
        password: userData.password,
      });

      if (userData.roleId === 5) {
        setViewerSelected(true);
        onLoadViewerVendors(userData.userId);
        onLoadCustomerVendors(
          superAdmin ? userData.customerId : currentUser.Claim_CustomerId,
        );
      }
    };

    const handleVendorDelete = async (vendorId) => {
      try {
        snackActions.success(`Successfully removed Vendor from list.`);
        const newViewerVendors = viewerVendors.filter(
          (vendor) => vendor.vendorId !== vendorId,
        );
        setViewerVendors(newViewerVendors);
        return true;
      } catch (err: any) {
        snackActions.error(err);
        return err;
      }
    };

    useEffect(() => {
      setForm(initialState);
      if (!superAdmin && userData.userId !== '') {
        handleSetEditForm();
      } else {
        setForm(initialState);
      }
    }, [userData]);

    if (!isUserModalOpen) return null;

    return (
      <Modal open={isUserModalOpen} onClose={() => handleCloseUserModal()}>
        <ModalBox>
          <ModalContent>
            {admin ? (
              // eslint-disable-next-line react/jsx-no-useless-fragment
              <>
                {!edit ? (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Create User ({currentLocationAndFacility.locationName})
                  </Typography>
                ) : (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Edit User ({currentLocationAndFacility.locationName})
                  </Typography>
                )}
              </>
            ) : (
              // eslint-disable-next-line react/jsx-no-useless-fragment
              <>
                {superAdmin && (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Create User
                  </Typography>
                )}
              </>
            )}

            {!showErrors ? (
              <>
                <Box
                  sx={{
                    width: '100%',
                    display: 'flex',
                    [theme.breakpoints.up('lg')]: {
                      flexDirection: 'row',
                    },
                    [theme.breakpoints.down('md')]: {
                      flexDirection: 'column',
                    },
                    gap: '16px',
                  }}
                >
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="First name"
                    value={form.firstName}
                    onChange={(value) => inputHandler('firstName', value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        onSaveNewUser();
                      }
                    }}
                  />
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="Last name"
                    value={form.lastName}
                    onChange={(value) => inputHandler('lastName', value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        onSaveNewUser();
                      }
                    }}
                  />
                </Box>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Username"
                  value={form.username}
                  onChange={(value) => inputHandler('username', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Email"
                  value={form.email}
                  onChange={(value) => inputHandler('email', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={userRoles}
                  size="small"
                  value={form.role}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('role', value);
                  }}
                  renderInput={(params) => (
                    <TextField {...params} label="Role" />
                  )}
                />
                {viewerSelected && (
                  <>
                    <Autocomplete
                      sx={{ width: '100%' }}
                      options={vendorSelectionOptions}
                      size="small"
                      value={selectedVendor}
                      onChange={(
                        event: any,
                        value: AutoCompleteOptionType | null,
                      ) => {
                        vendorInputHandler('vendor', value);
                      }}
                      renderInput={(params) => (
                        <TextField {...params} label="Vendor" />
                      )}
                    />
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'row',
                        flexWrap: 'wrap',
                        width: '100%',
                        gap: '16px',
                      }}
                    >
                      {viewerVendors.map((vendor) => (
                        <Chip
                          key={vendor.vendorId}
                          label={`${vendor.name}`}
                          variant="outlined"
                          onDelete={(event: any) =>
                            handleVendorDelete(vendor.vendorId)
                          }
                        />
                      ))}
                    </Box>
                  </>
                )}
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Password"
                  value={form.password}
                  onChange={(value) => inputHandler('password', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
              </>
            ) : (
              <>
                <Box
                  sx={{
                    width: '100%',
                    display: 'flex',
                    [theme.breakpoints.up('lg')]: {
                      flexDirection: 'row',
                    },
                    [theme.breakpoints.down('md')]: {
                      flexDirection: 'column',
                    },
                    gap: '16px',
                  }}
                >
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="First name"
                    value={form.firstName}
                    error={formErrors.firstName}
                    onChange={(value) => inputHandler('firstName', value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        onSaveNewUser();
                      }
                    }}
                  />
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="Last name"
                    value={form.lastName}
                    error={formErrors.lastName}
                    onChange={(value) => inputHandler('lastName', value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') {
                        onSaveNewUser();
                      }
                    }}
                  />
                </Box>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Username"
                  value={form.username}
                  error={formErrors.username}
                  onChange={(value) => inputHandler('username', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Email"
                  value={form.email}
                  error={formErrors.email}
                  onChange={(value) => inputHandler('email', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={userRoles}
                  size="small"
                  value={form.role}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('role', value);
                  }}
                  renderInput={(params) =>
                    formErrors.role ? (
                      <TextField
                        {...params}
                        error
                        helperText={formErrors.role}
                        label="Role"
                      />
                    ) : (
                      <TextField {...params} label="Role" />
                    )
                  }
                />
                {viewerSelected && (
                  <>
                    <Autocomplete
                      sx={{ width: '100%' }}
                      options={vendorSelectionOptions}
                      size="small"
                      value={selectedVendor}
                      onChange={(
                        event: any,
                        value: AutoCompleteOptionType | null,
                      ) => {
                        vendorInputHandler('vendor', value);
                      }}
                      renderInput={(params) => (
                        <TextField {...params} label="Vendor" />
                      )}
                    />
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'row',
                        flexWrap: 'wrap',
                        width: '100%',
                        gap: '16px',
                      }}
                    >
                      {viewerVendors.map((vendor) => (
                        <Chip
                          key={vendor.vendorId}
                          label={`${vendor.name}`}
                          variant="outlined"
                          onDelete={(event: any) =>
                            handleVendorDelete(vendor.vendorId)
                          }
                        />
                      ))}
                    </Box>
                  </>
                )}
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Password"
                  value={form.password}
                  error={formErrors.password}
                  onChange={(value) => inputHandler('password', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewUser();
                    }
                  }}
                />
              </>
            )}
            <Typography variant="subtitle2" color="#d32f2f">
              {error}
            </Typography>
            <Box
              sx={{
                display: 'flex',
                width: '100%',
                [theme.breakpoints.up('sm')]: {
                  justifyContent: 'flex-end',
                },
                [theme.breakpoints.down('sm')]: {
                  justifyContent: 'center',
                },
                gap: '8px',
              }}
            >
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="text"
                size="large"
                onClick={() => handleCloseUserModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewUser()}
              >
                Submit
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
