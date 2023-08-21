import classNames from 'classnames';
import React from 'react';

import styles from './Table.module.scss';

const Table = ({
    columns,
    data = [],
    className,
}) => {
    return (
        <table className={classNames(styles.airdropTable, className)}>
            <thead>
                <tr>
                    {columns.map(el => (
                        <th
                            style={{
                                textAlign: el.align || 'left',
                                width: el.width ?? 'auto',
                            }}
                        >
                            {el.title}
                        </th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {data.map((rowData, rowIndex) => (
                    <tr key={rowIndex}>
                        {columns.map((colConfig, colIndex) => {
                            const colData = rowData[colConfig.key];

                            return (
                                <td
                                    key={colIndex}
                                    style={{
                                        textAlign: colConfig.align || 'left',
                                        width: colConfig.width ?? 'auto',
                                    }}
                                >
                                    {
                                        colConfig.render
                                            ? colConfig.render(colData, rowData)
                                            : colData
                                    }
                                </td>
                            );
                        })}
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default Table;