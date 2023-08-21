// import LinesEllipsis from 'react-lines-ellipsis';

// const EllipsisText = ({ text, maxLine = 1, className, basedOn = 'letters' }) => {
//     return (
//         <LinesEllipsis
//             className={className}
//             text={text}
//             maxLine={maxLine}
//             ellipsis='...'
//             basedOn={basedOn}
//         />
//     )
// }

// export default EllipsisText;

import styles from './index.module.scss';

const EllipsisText = ({ text, className, maxLine = 1 }) => {
    return (
        <span className={`${className || ''} ${styles.ellipsisWrapper}`}>
            <span
                className={styles.ellipsisText}
                style={{
                    WebkitLineClamp: maxLine
                }}
            >
                { text }
            </span>
        </span>
    )
}

export default EllipsisText;
