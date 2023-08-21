import { sportTypes } from '@constants';

import FootballIcon from '@assets/icons/football.svg';
import TennisIcon from '@assets/icons/tennis.svg';

const sportIcons = {
    [sportTypes.SOCCER]: FootballIcon,
    [sportTypes.TENNIS]: TennisIcon
}

const SportIcon = ({ id, sportType, className }) => {
    const SIcon = sportIcons[sportType];
    return <SIcon className={className} />
}

export default SportIcon;
