import { useSelector } from 'react-redux';
import React, { useEffect, useRef, useState } from 'react';
import classNames from 'classnames';
import { useMemo } from 'react';
import { FormattedNumber } from 'react-intl';
import { useDispatch } from 'react-redux';
import { Tooltip } from 'react-tooltip';

import { walletTabs } from '@constants/masterData';
import { accountActions, coinActions } from '@redux/actions';
import { useNotification } from '@hooks';

import TabLayout from '../TabLayout';
import { BasicForm, Button, InputTextField, SelectField, InputNumericField, Loading } from '@components/Common';

import WalletIcon from '@assets/icons/wallet-menu.svg';
import ConnectSquareIcon from '@assets/icons/connect-square.svg';
import InfoCircleIcon from '@assets/icons/info-circle.svg';

import styles from './index.module.scss';

const SingleValue = ({ children, ...props }) => {
    return (
        <SelectField.components.SingleValue {...props} className={styles.coinItem}>
            <img src={`/images/tokens/${children}.png`} />
            {children}
        </SelectField.components.SingleValue>
    );
}

const OptionItem = props => {
    return (
        <SelectField.components.Option {...props} className={classNames(styles.coinItem, styles.option)}>
            <img src={`/images/tokens/${props.label}.png`} />
            {props.label}
        </SelectField.components.Option>
    );
};

const selectCustomStyles = {
    control: () => ({
        borderRadius: 4,
    }),
    indicator: () => ({
        marginRight: 0,
    }),
    indicatorsContainer: () => ({
        '> *': {
            padding: '8px 16px !important',
        }
    }),
    valueContainer: () => ({
        padding: '0 16px',
        minHeight: '48px',
    }),
}

const BCSwap = () => {
    const formRef = useRef();
    const refDebounceCalc = useRef();
    const { balances } = useSelector(state => state.account);
    const [swapFrom, setSwapFrom] = useState('');
    const [swapTo, setSwapTo] = useState('');
    const [loadingCalc, setLoadingCalc] = useState(false);
    const [loadingSwap, setLoadingSwap] = useState(false);
    const [coinSrcAmount, setCoinSrcAmount] = useState(0);
    const [coinDstAmount, setCoinDstAmount] = useState(0);
    const dispatch = useDispatch();
    const { showError, showSuccess } = useNotification();

    const coinFrom = useMemo(() => {
        return balances?.find(el => el.id === swapFrom);
    }, [swapFrom]);
    const coinTo = useMemo(() => {
        return balances?.find(el => el.id === swapTo);
    }, [swapTo]);

    const swapRatio = useMemo(() => {
        if (swapFrom && swapTo && coinDstAmount && coinSrcAmount) {
            return coinDstAmount / coinSrcAmount;
        }

        return 0;
    }, [coinDstAmount])

    const handleSubmit = values => {
        setLoadingSwap(true);
        dispatch(coinActions.swapCoin({
            params: values,
            onCompleted: res => {
                showSuccess("Submit swap coin successful!");
                setLoadingSwap(false);
                formRef.current.resetForm();
                setCoinSrcAmount(0);
                setCoinDstAmount(0);
                setSwapFrom(0);
                setSwapTo(0);
                dispatch(accountActions.getBalance());
            },
            onError: (e) => {
                setLoadingSwap(false);
                showError('Submit swap coin failed!');
            }
        }));
    }

    const handleSwapCoinTypeSelected = () => {
        if (swapFrom || swapTo) {
            formRef.current.setFieldValue('src_coin', swapTo);
            formRef.current.setFieldValue('dst_coin', swapFrom);
            formRef.current.setFieldValue('amount', 0);
            setCoinSrcAmount(0);
            setCoinDstAmount(0);
            setSwapFrom(swapTo);
            setSwapTo(swapFrom);
        }
    }

    const handleUseMaxCurrentcoinSrcAmount = () => {
        const coinFrom = balances?.find(el => el.id === swapFrom);
        if (coinFrom?.amount) {
            formRef.current.setFieldValue('amount', coinFrom?.amount);
            setCoinSrcAmount(+coinFrom?.amount);
        }
    }

    const handleBlurAmount = e => {
        let amount = +(e.target.value || '').replace(/,/g, '');
        const coinFrom = balances?.find(el => el.id === swapFrom);
        const maxAmountCoinForm = +coinFrom?.amount;
        if (maxAmountCoinForm && maxAmountCoinForm < amount) {
            amount = maxAmountCoinForm;
            formRef.current.setFieldValue('amount', maxAmountCoinForm);
        }

        setCoinSrcAmount(amount);
    }

    useEffect(() => {
        if (coinSrcAmount && swapFrom && swapTo) {
            setLoadingCalc(true);
            refDebounceCalc.current = setTimeout(() => {
                dispatch(coinActions.calcSwapAmount({
                    params: {
                        src_coin: swapFrom,
                        dst_coin: swapTo,
                        amount: coinSrcAmount,
                    },
                    onCompleted: res => {
                        setCoinDstAmount(res.data?.amount);
                        setLoadingCalc(false);
                    },
                    onError: () => {
                        setLoadingCalc(false);
                    }
                }));
            }, 300);
        }

        return () => {
            if (refDebounceCalc.current) {
                clearTimeout(refDebounceCalc.current);
            }
        }
    }, [coinSrcAmount, swapFrom, swapTo]);

    return (
        <TabLayout
            tabs={walletTabs}
            title="Wallet"
            icon={<WalletIcon />}
        >
            <div className={styles.bcSwap}>
                <div className={styles.header}>
                    <div className={styles.title}>You get Approximately</div>
                </div>

                <BasicForm
                    initialValues={{
                        src_coin: '',
                        dst_coin: '',
                        receiver_wallet: '',
                        amount: 0,
                    }}
                    id="swapForm"
                    onSubmit={handleSubmit}
                    formikRef={formRef}
                >
                    <div className={styles.swapContainer}>
                        <div className={styles.swapList}>
                            <div className={styles.swapItem}>
                                <SelectField
                                    options={balances.filter(opt => opt.id !== swapTo)}
                                    name="src_coin"
                                    isSearchable={false}
                                    className={styles.select}
                                    optionLabelKey="name"
                                    optionValueKey="id"
                                    selectStyles={selectCustomStyles}
                                    customComponents={{
                                        Option: OptionItem,
                                        SingleValue,
                                    }}
                                    onChange={({ value }) => setSwapFrom(value)}
                                    placeholder="Select coin"
                                />
                                <button
                                    type="button"
                                    className={styles.badge}
                                    onClick={handleUseMaxCurrentcoinSrcAmount}
                                >
                                    MAX
                                </button>
                                <div className={styles.swapValue}>
                                    <label>Send</label>
                                    <InputNumericField
                                        placeholder=""
                                        name="amount"
                                        pattern="^\d*(\.\d{0,6})?$"
                                        className={styles.inputAmount}
                                        onBlur={handleBlurAmount}
                                    />
                                </div>
                            </div>
                            <div className={styles.swapItem}>
                                <SelectField
                                    options={balances.filter(opt => opt.id !== swapFrom)}
                                    name="dst_coin"
                                    isSearchable={false}
                                    className={styles.select}
                                    optionLabelKey="name"
                                    optionValueKey="id"
                                    selectStyles={selectCustomStyles}
                                    customComponents={{
                                        Option: OptionItem,
                                        SingleValue,
                                    }}
                                    onChange={({ value }) => setSwapTo(value)}
                                    placeholder="Select coin"
                                />
                                <div className={styles.swapValue}>
                                    <label>Get</label>
                                    <div className={styles.valueTo}>
                                        {loadingCalc ? (
                                            <Loading size={16} />
                                        ) : (
                                            <FormattedNumber value={coinDstAmount || 0} maximumFractionDigits="2" />
                                        )}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <button type="button" onClick={handleSwapCoinTypeSelected} className={styles.btnSwapCoinType}><ConnectSquareIcon /></button>
                    </div>
                    <div className={styles.withdraw}>
                        <div className={styles.title}>
                            Receiver address
                            <InfoCircleIcon data-tooltip-id="hintReceiverAddress" />
                            <Tooltip
                                id="hintReceiverAddress"
                                place="top"
                                content="If this field is not filled, then deposit address will be receiver address."
                                className={styles.hintReceiverAddress}
                            />
                        </div>
                        <InputTextField
                            placeholder="Fill in carefully according to the specific currency"
                            name="receiver_wallet"
                            className={styles.input}
                        />
                    </div>
                </BasicForm>
                <div className={styles.tips}>
                    <p>
                        {!!swapRatio && `1 ${coinFrom?.name} ≈ ${swapRatio} ${coinTo?.name}`}
                    </p>
                    <p>Swap fee: <b>0.2</b> ADA</p>
                </div>
                <div>
                    <Button
                        type="submit"
                        form="swapForm"
                        disabled={!coinSrcAmount || !coinDstAmount || loadingSwap}
                        className={styles.btnSwap}
                    >
                        Swap Now
                        {loadingSwap && <Loading size={16} style={{ marginLeft: 8 }} />}
                    </Button>
                    <div className={styles.swapHint}>
                        After do swap, your transaction will be submitted to chain after a while.
                    </div>
                </div>
            </div>
        </TabLayout>
    );
};

export default BCSwap;