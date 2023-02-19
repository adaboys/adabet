const Flexbox = ({
    direction = 'row',
    spacing,
    justify,
    align,
    ...props
}) => (
    <div
        style={{
            display: 'flex',
            flexDirection: direction,
            ...(spacing && { gap: spacing }),
            ...(justify && { justifyContent: justify }),
            ...(align && { alignItems: align })
        }}
        {...props}
    >

    </div>
)

export default Flexbox;
