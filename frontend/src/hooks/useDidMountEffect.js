import { useEffect, useRef } from 'react'

const useDidMountEffect = (func, deps) => {
    const didMount = useRef(false)

    useEffect(() => {
        if (didMount.current) {
            return func()
        } else {
            didMount.current = true
        }
    }, deps)
}

export default useDidMountEffect
