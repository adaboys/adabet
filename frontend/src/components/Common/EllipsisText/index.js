import LinesEllipsis from 'react-lines-ellipsis';

const EllipsisText = ({ text, maxLine = 1, className, basedOn = 'letters' }) => {
    return (
        <LinesEllipsis
            className={className}
            text={text}
            maxLine={maxLine}
            ellipsis='...'
            // trimRight={false}
            basedOn={basedOn}
        />
    )
}

export default EllipsisText;
