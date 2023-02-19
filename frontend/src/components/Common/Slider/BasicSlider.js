import Carousel from "react-multi-carousel";
import "react-multi-carousel/lib/styles.css";

import styles from './BasicSlider.module.scss';

const BasicSlider = ({ children, desktopItems = 1, mobileItems = 1, tabletItems = 1, ...props }) => {
    const responsive = {
        desktop: {
            breakpoint: { max: 3000, min: 1024 },
            items: desktopItems,
            slidesToSlide: 1,
            partialVisibilityGutter: 40
        },
        tablet: {
            breakpoint: { max: 1024, min: 464 },
            items: tabletItems,
            slidesToSlide: 1,
            partialVisibilityGutter: children?.length > tabletItems ? 30 : 0
        },
        mobile: {
            breakpoint: { max: 464, min: 0 },
            items: mobileItems,
            slidesToSlide: 1, // optional, default to 1.
            partialVisibilityGutter: children?.length > mobileItems ? 20 : 0
        }
    };

    return (
        <Carousel
            responsive={responsive}
            removeArrowOnDeviceType={['tablet', 'mobile']}
            className={styles.basicSlider}
            {...props}
        >
            { children }
        </Carousel>
    );
}

export default BasicSlider;

