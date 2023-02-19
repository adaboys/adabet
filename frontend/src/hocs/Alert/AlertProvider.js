import { transitions, positions, Provider } from 'react-alert';

import AlertTemplate from './AlertTemplate';

// optional configuration
const alertOptions = {
    // you can also just use 'bottom center'
    position: positions.TOP_RIGHT,
    timeout: 2000,
    // offset: '30px', --> for margin
    // you can also just use 'scale'
    transition: transitions.FADE,
}

const AlertProvider = ({ children }) => {
    return (
        <Provider template={AlertTemplate} {...alertOptions}>
            {children}
        </Provider>
    )
}

export default AlertProvider;
