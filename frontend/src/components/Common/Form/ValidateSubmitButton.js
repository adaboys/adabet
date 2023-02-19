import { useFormikContext } from 'formik';

import Button from '../Button';
import { isEmptyObject } from '@utils';

const ValidateSubmitButton = ({
    text,
    formRef,
    ...props
}) => {
    const { errors, dirty } = useFormikContext();
    const disabled = !dirty || !isEmptyObject(errors);
    return (
        <Button disabled={disabled} {...props}>{text}</Button>
    );
};

export default ValidateSubmitButton;