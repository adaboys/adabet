import GeneralInfoForm from "./GeneralInfoForm";

import { useAuth } from "@hooks";

import DetailLayout from "./DetailLayout";

const AccountDetail = () => {
    const { user } = useAuth();

    return (
        <DetailLayout>
            <GeneralInfoForm user={user} />
        </DetailLayout>
    )
}

export default AccountDetail;