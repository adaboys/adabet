import { useRouter } from "next/router";
import { useEffect } from "react";

const Redirect = ({ options, as, url }) => {
    const { push } = useRouter();

    useEffect(() => {
        push(url, as, options);
    }, []);

    return <></>;
};

export default Redirect;
