import { defineMessages, useIntl } from 'react-intl';
import { useNotification } from '@hooks';
import classNames from 'classnames';

const messages = defineMessages({
    copySuccess: 'Copy successful!'
});

const CopyWrapper = ({ children, text, className }) => {
    const intl = useIntl();
    const { showSuccess } = useNotification();

    const onCopy = () => {
        if(text) {
            navigator.clipboard.writeText(text);
            showSuccess(intl.formatMessage(messages.copySuccess));
        }
    }
    return (
        <span onClick={onCopy} className={classNames("cursor-pointer", {[className]: !!className})}>
            { children }
        </span>
    )
}

export default CopyWrapper;