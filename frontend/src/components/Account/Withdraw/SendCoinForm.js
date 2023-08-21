import { defineMessages, useIntl } from 'react-intl';
import * as Yup from 'yup';
import { useState, useRef, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import {
    InputNumericField,
    InputTextField,
    ValidateSubmitButton,
    BasicForm,
    Flexbox,
    Radio
} from '@components/Common';
import Coin from './Coin';

import { commonMessages, validationMessages } from '@constants/intl';
import { useNotification } from '@hooks';
import { accountActions } from '@redux/actions';
import { coinTypes } from '@constants';

import styles from './index.module.scss';

const messages = defineMessages({
    amountLabel: 'Amount',
    amountPlaceholder: '0.000000',
    amountInvalid: 'Amount must be greater than 1.4',
    receiverId: 'Receiver’s address',
    transferCoinFailed: 'Transfer coin failed!',
    transferCoinBalanceNotEnough: 'Balance not enough!'
});


const SendCoinForm = ({ onSendCoinSuccess }) => {
    const intl = useIntl();
    const dispath = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [currencyId, setCurrencyId] = useState(null);

    const { showPopupError } = useNotification();

    const formikRef = useRef(null);

    const { balances } = useSelector(state => state.account);

    const onSubmit = (values) => {
        setIsSubmitting(true);
        const params = {
            ...values,
            currency_id: currencyId
        }
       
        dispath(accountActions.sendConfirmCoin({
            params,
            onCompleted: (result) => {
                if (result?.status === 200) {
                    onSendCoinSuccess(params);
                }
                else if (result?.code === 'balance_not_enough') {
                    showPopupError(intl.formatMessage(messages.transferCoinBalanceNotEnough));
                }
                else {
                    showPopupError(intl.formatMessage(messages.transferCoinFailed));
                }
                setIsSubmitting(false);
            },
            onError: (result) => {
                showPopupError(intl.formatMessage(messages.transferCoinFailed));
                setIsSubmitting(false);
            }
        }))
    }

    useEffect(() => {
        if(balances?.length) {
            setCurrencyId(balances[0].id);
        }
    }, [balances])

    return (
        <BasicForm
            formikRef={formikRef}
            initialValues={{
                amount: 1.4,
                receiver_address: ''
            }}
            validationSchema={Yup.object().shape({
                receiver_address: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
                amount: Yup.number()
                    .required(intl.formatMessage(validationMessages.required))
                    .min(1.4, intl.formatMessage(messages.amountInvalid))
            })}
            onSubmit={onSubmit}
        >
            <Flexbox spacing="16px" style={{marginBottom: '16px'}}>
                {
                    balances.map(coin => (
                        <Radio
                            onChange={() => setCurrencyId(coin.id)}
                            checked={currencyId === coin.id}
                        >
                            <Coin type={coin.name.toLowerCase()} />
                        </Radio>
                    ))
                }
                {/* <Radio
                    onChange={() => setAmountType(coinTypes.ADA)}
                    checked={amountType === coinTypes.ADA}
                >
                    <Coin type={coinTypes.ADA} />
                </Radio>
                <Radio
                    onChange={() => setAmountType(coinTypes.ABE)}
                    checked={amountType === coinTypes.ABE}
                >
                    <Coin/>
                </Radio>
                <Radio
                    onChange={() => setAmountType(coinTypes.GEM)}
                    checked={amountType === coinTypes.GEM}
                >
                    <Coin type={coinTypes.GEM}/>
                </Radio> */}
            </Flexbox>
            <InputNumericField
                label={intl.formatMessage(messages.amountLabel)}
                placeholder={intl.formatMessage(messages.amountPlaceholder)}
                name="amount"
                pattern="^\d*(\.\d{0,6})?$"
                // iconRight={<Coin type={coinTypes.ADA} style={{ position: 'absolute', top: '50%', right: 0,  transform: 'translateY(-50%)' }} />}
            />

            <InputTextField
                label={intl.formatMessage(messages.receiverId)}
                name="receiver_address"
            />

            <ValidateSubmitButton
                className={styles.btnUpdate}
                text={intl.formatMessage(commonMessages.confirm)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    )
}

export default SendCoinForm;