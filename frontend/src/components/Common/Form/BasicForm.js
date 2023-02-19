import React from 'react';
import { Formik, Form } from 'formik';

const BasicForm = ({
    id,
    formikRef,
    children,
    className,
    initialValues,
    validationSchema,
    onSubmit,
    onChange
}) => (
    <Formik
        initialValues={initialValues}
        onSubmit={onSubmit}
        validationSchema={validationSchema}
        innerRef={formikRef}
    >
        {() => (
            <Form id={id} className={className} onChange={onChange}>
                {children}
            </Form>
        )}
    </Formik>
);

export default BasicForm;
