const Flexbox = ({
    direction = 'row',
    spacing,
    justify,
    align,
    children,
    disabled,
    style={},
    onClick,
    ...props
}) => {
    
    return (
        <div
            style={{
                display: 'flex',
                flexDirection: direction,
                ...(spacing && { gap: spacing }),
                ...(justify && { justifyContent: justify }),
                ...(align && { alignItems: align }),
                ...(disabled && { cursor: 'not-allowed' }),
                ...style
            }}
            onClick={disabled ? null : onClick}
            {...props}
        >
            {children}
        </div>
    )
}

export default Flexbox;
