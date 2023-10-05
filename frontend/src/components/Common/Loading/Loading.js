import classNames from "classnames";
import MoonLoader from "react-spinners/MoonLoader";

const Loading = ({ style, className, size }) => (
    <div className={classNames("d-flex align-items-center justify-content-center text-align-left", className)} style={style}>
        <MoonLoader color="white" loading size={size || 30}/>
    </div>
)

export default Loading;
